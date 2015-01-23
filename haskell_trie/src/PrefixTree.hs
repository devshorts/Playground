module PrefixTree where

import qualified Data.List as L
import Data.Maybe
import Control.Monad
import qualified Data.Foldable as Fold

type Key a = [a]

data Trie key = Node (Maybe key) [Trie key] Bool deriving (Show, Eq, Read)

empty :: [Trie key]
empty = []

{-
    Finds a key in the trie list that matches the target key
-}
findKey :: (Eq t) => t -> [Trie t] -> Maybe (Trie t)
findKey key tries = L.find (\(Node next _ _) -> next == Just key) tries

{-  
    Takes a key list and finds the trie that fullfils that prefix
-}
findTrie :: (Eq t) => Key t -> [Trie t] -> Maybe (Trie t)
findTrie [] _ = Nothing
findTrie (x:[]) tries = findKey x tries 
findTrie (x:xs) tries = findKey x tries >>= nextTrie
    where nextTrie (Node _ next _) = findTrie xs next

exists :: (Eq t) => Key t -> [Trie t] -> Maybe Bool
exists keys trie = findTrie keys trie >>= \(Node _ _ isWord) -> 
    if isWord then return isWord 
    else Nothing
                
insert :: (Eq t) => Key t -> [Trie t] -> [Trie t]
insert [] t = t
insert (x:xs) tries = 
    case findKey x tries of 
        Nothing -> [(Node (Just x) (insert xs [])) isEndWord]++tries
        Just value -> 
            let (Node key next word) = value
            in [Node key (insert xs next) (toggleWordEnd word)]++(except value)
    where 
        except value = (L.filter ((/=) value) tries)
        isEndWord = if xs == [] then True else False
        toggleWordEnd old = if xs == [] then True else old

{- 
    Counts the compressed size of the trie 
-}
countChars :: [Trie t] -> Integer
countChars trie = count trie 0
    where 
        count [] num = num
        count ((Node _ next _):xs) num = 
            count (xs++next) (num + 1)


{- 
    Gives you all the available words in the trie list
-}
allWords :: [Trie b] -> [[b]]
allWords tries = 
    let raw = rawWords tries
    in map (flatMap id) raw
    where 
        flatMap f = Fold.concatMap (Fold.toList . f)
        rawWords tries = [key:next
                            | (Node key suffixes isWord) <- tries
                            , next <- 
                                if isWord then 
                                    []:(rawWords suffixes)
                                else 
                                    rawWords suffixes]

{- 
    This function takes a key list and returns the 
    full words that match the key list. But, since we already 
    know about the source key that matches (the input) we can
    prepend that information to any suffix information that is left.
    If the node found that matches the original query is a word in itself
    we can prepend that too since its a special case
-}
guess :: (Eq a) => Key a -> [Trie a] -> Maybe [Key a]
guess word trie = 
    findTrie word trie >>= \(Node _ next isWord) -> 
    return $ (source isWord) ++ (prependOriginal word $ allWords next)
    where 
        source isWord = if isWord then [word] else []
        prependOriginal word = map (\elem -> word ++ elem)
    
{-
    Helper to build a prefix trie from a dictionary
-}
build :: (Eq t) => [Key t] -> [Trie t]
build list = buildTrie list empty
    where 
        buildTrie [] trie = trie
        buildTrie (x:xs) trie = buildTrie xs (insert x trie)