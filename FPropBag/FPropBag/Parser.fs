namespace FPropEngine

module Parser = 

    open FParsec

    type Ast = 
        | Bag of string list
        | Literals of string
        | ForLoop of string * Ast * Ast list

    let private tokenPrefix = '$'

    let reconstructLiteral list = 
        match list with 
            | [] -> ""
            | h::t ->
                seq{
                    yield tokenPrefix |> string
                    yield h
                    for item in t do
                        yield "."
                        yield item
                } |> Seq.reduce (+)

    let private tagStart = pstring (string tokenPrefix)

    let private token n = tagStart >>. pstring n |>> ignore 

    let private tagDelim = eof <|> spaces1

    let private endTag = token "end" 
    let private forTag = token "for" 

    let private languageSpecific = [attempt endTag; forTag] |> List.map (fun i -> i .>> tagDelim)

    let private anyReservedToken = attempt (languageSpecific |> List.reduce (<|>))

    let private tokenable = many1Chars (satisfy isDigit <|> satisfy isLetter)

    let private element = attempt (tokenable .>> pstring ".") <|> tokenable

    let private nonTokens = many1Chars (satisfy (isNoneOf [tokenPrefix])) |>> Literals

    let private bag = tagStart >>. many1 element |>> Bag

    let private innerElement = notFollowedBy anyReservedToken >>. (nonTokens <|> bag)

    let private tagFwd, tagImpl = createParserForwardedToRef()

    let private forLoop = parse {
        do! spaces
        do! forTag
        do! spaces
        do! skipAnyOf "$"
        let! alias = tokenable
        do! spaces
        let! _ = pstring "in"
        do! spaces
        let! elements = bag
        do! spaces
        let! body = many tagFwd
        do! spaces
        do! endTag
        do! spaces
        return ForLoop (alias, elements, body) 
    } 

    tagImpl := attempt forLoop <|> innerElement

    let get str = 
        match run (many tagFwd) str with
             | Success(r, _, _) -> r 
             | Failure(r,_,_) -> failwith "nothing"

      
