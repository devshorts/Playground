module Rockpaper where

import System.Random
import Control.Monad
import Data.List 
import Control.Arrow

data Moves = Rock | Paper | Scissors deriving (Show, Read, Eq, Ord)

game :: Moves -> Moves -> Bool
game Paper Rock  = True
game Scissors Paper = True
game Rock Scissors = True
game _ _ = False

allMoves :: [Moves]
allMoves = [Rock, Paper, Scissors]

pick :: Int -> Moves
pick n = allMoves !! (n `mod` length allMoves)

computerMoves :: (RandomGen a) => a -> [IO Moves]
computerMoves seed = do
	let (rand, nSeed) = randomR (0, length allMoves) seed
	let next = pick (fromIntegral rand)
	(return next) : (computerMoves nSeed)


humanPlayer :: IO Moves
humanPlayer  = do
	putStr "Play: "
	input <- getLine
	return (read input::Moves)

play = do
	seed <- newStdGen
	compMov <- head $ computerMoves seed
	humanMov <- humanPlayer
	let gameResult = game humanMov compMov
		in do
			putStrLn $ "Your move " ++ (show humanMov) 
								    ++ ", computer: " 
								  	++ (show compMov) 
								  	++ ", " 
								  	++ (show gameResult)
		  	return gameResult


main = do
    putStr "how many games? "
    num <- fmap (\x -> read x::Int) getLine
    games <- sequence $ take num $ repeat play
    let winTypes = map (head &&& length) (group games)
        wins = filter fst winTypes
        losses = filter (not . fst) winTypes
        totalWins = sum $ map snd  wins
        totalLosses = sum $ map snd losses
    putStrLn ("Max consecutive wins : " ++ (show $ snd $ maximum wins))
    putStrLn ("Max consecutive loses : " ++ (show $ snd $ maximum losses))
    putStrLn ("Max total wins  : " ++ (show totalWins))
    putStrLn ("Max total losses : " ++ (show totalLosses))
