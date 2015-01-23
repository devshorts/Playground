namespace FPropBag.Tests

module Tests = 
    open FsUnit
    open NUnit.Framework

    open FPropEngine.Formatter
    open FPropEngine

    [<Test>]
    let testNoBag () =
        let ctx = new Context();
        
        let text = "$nothing.here"

        let parsed = Runner.run ctx text

        parsed |> should equal text


    [<Test>]
    let testBag () =
        let ctx = new Context();

        let somethingCtx = new Context()

        somethingCtx.add("here", "foo")

        ctx.add("something", somethingCtx)
        
        let text = "$something.here"

        let parsed = Runner.run ctx text

        parsed |> should equal "foo"


    [<Test>]
    let testLoop () =
        let ctx = new Context();

        let somethingCtx = new Context()

        somethingCtx.add("here", "foo")

        ctx.add("something", somethingCtx)
        
        let text = "$for $item in $something.here $item!!$end"

        let parsed = Runner.run ctx text

        parsed |> should equal "foo!!"


