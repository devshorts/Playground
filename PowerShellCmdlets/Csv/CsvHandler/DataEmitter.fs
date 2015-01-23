namespace CsvHandler

open System
open System.Reflection
open System.Reflection.Emit

module DataEmitter = 
    type DynamicField = {
        Name : String;
        Type : Type;
        Value: obj;
    }
        
    type FieldName = string
    type TypeName = string

    (* encapsulates an incrementable index *)
    type IncrementingCounterBuilder () = 
        let mutable start = 0
        member this.Return(expr) = 
            start <- start + 1            
            expr start 
              
    (* Handles automatically passing the il generator through the requested calls *)
    type ILGenBuilder (gen: ILGenerator) = 
        member this.Bind(expr, func)= 
            expr gen
            func () |> ignore

        member this.Return(v) = ()
        member this.Zero () = ()
        member this.For(col, func) = for item in col do func item
        member this.Combine expr1 expr2 = ()
        member this.Delay expr = expr()
   
    let private assemblyName = new AssemblyName("Dynamics")

    let private assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave)

    let private moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll")

    let private typeBuilder typeName = moduleBuilder.DefineType(typeName, TypeAttributes.Public)

    let private fieldBuilder (typeBuilder:TypeBuilder) name fieldType : FieldBuilder = 
        typeBuilder.DefineField(name, fieldType, FieldAttributes.Public)

    let private createConstructor (typeBuilder:TypeBuilder) typeList =
        typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, typeList |> List.toArray)

    let private loadThis (gen: ILGenerator) = 
        gen.Emit(OpCodes.Ldarg_0)

    let private emitReturn (gen: ILGenerator) = 
        gen.Emit(OpCodes.Ret)

    let private callDefaultConstructor (gen: ILGenerator) = 
        gen.Emit(OpCodes.Call, typeof<obj>.GetConstructor(Type.EmptyTypes))        
    
    let private setFieldFromStack (field : FieldBuilder) (gen : ILGenerator) = 
        gen.Emit(OpCodes.Stfld, field)
    
    let private loadArgToStack (argIndex : int) (gen: ILGenerator) = 
        gen.Emit(OpCodes.Ldarg, argIndex)

    let private build (fields : FieldBuilder list) (cons : ConstructorBuilder) = 
        let generator = cons.GetILGenerator()

        let ilBuilder = new ILGenBuilder(generator)

        let forNextIndex = new IncrementingCounterBuilder()

        ilBuilder {   
            do! loadThis
            do! callDefaultConstructor
            do! loadThis

            for field in fields do
                do! loadThis
                do! forNextIndex { return loadArgToStack }
                do! field |> setFieldFromStack  
                
            do! emitReturn
        }

    let make (name : TypeName) (types : (FieldName * Type) list)= 
        let typeBuilder = typeBuilder name
        let fieldBuilder = fieldBuilder typeBuilder
        let createConstructor = createConstructor typeBuilder        
        let fields = types |> List.map (fun (name, ``type``) -> fieldBuilder name ``type``)
        let definedConstructor = types |> List.map snd |> createConstructor
    
        definedConstructor |> build fields

        typeBuilder.CreateType()

    let instantiate (typeName : TypeName) (objInfo : DynamicField list) =
        let values = objInfo |> List.map (fun i -> i.Value) |> List.toArray
        let types  = objInfo |> List.map (fun i -> (i.Name, i.Type))

        let t = make typeName types

        Activator.CreateInstance(t, values)