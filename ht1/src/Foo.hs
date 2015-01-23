module Foo where

import Control.Monad
import Common.Utils
import Data.List
import Data.Maybe

newtype Price = Price { getPrice :: Float } deriving (Show, Eq)

data Buyable = Meat | Fruit  deriving (Show, Eq)

data Coupon = 
        All Float
    |   Next Int Float   
    |   Nth Int Buyable Float deriving (Show, Eq)

data Cart = 
        Discount Coupon 
    |   Product Buyable Price
    deriving (Show, Eq)

cartList :: [Cart]
cartList =  [Product Fruit $ Price 2.0, 
            Discount $ All 0.5,
            Discount $ Nth 1 Meat 0.5,
            Product Meat $ Price 1.0,
            Product Meat $ Price 2.0]

updateCart ::  Float -> Cart -> Cart
updateCart percent (Product buyable price) = Product buyable newPrice
    where 
        newPrice = Price $ (getPrice price) * percent
updateCart _ coupon = coupon
 
isProduct :: Cart -> Bool
isProduct (Product _ _) = True
isProduct (Discount _)  = False 
 
targetMatches :: Buyable -> Cart -> Bool
targetMatches target (Product b _) = target == b
targetMatches _ _ = False
 

nextIndex :: Int -> (a -> Bool) -> [a] -> Maybe Int
nextIndex start predicate list = index  
    where 
        mIndex = findIndex predicate . drop (start ) $ list
        index  = mIndex >>= \idx -> return (idx + start)

getNth :: Int -> Int -> (a -> Bool) -> [a] -> Maybe Int
getNth start count predicate list 
    | count == 0 = index       
    | count /= 0 = index >>= next >>= continue
        where 
            index = nextIndex start predicate list
            next idx = return (idx + 1)
            continue idx = getNth idx (count - 1) predicate list

updateByPredicate :: [Cart] -> Int -> Int -> (Cart -> Bool) -> Float -> [Cart]
updateByPredicate list nth offset predicate percent = 
    case getNth offset nth predicate list of
        Just idx -> let (notProcessed, targetItem:remaining) = splitAt idx list                                                            
                    in notProcessed++[updateCart percent targetItem]++remaining
        Nothing -> list

procElem :: Cart -> [Cart] -> Int -> [Cart]
procElem (Discount(All percent)) list offset = 
    map (updateCart percent) list

procElem (Discount(Next nth percent)) list offset =  
    updateByPredicate list (nth-1) offset isProduct percent

procElem (Discount(Nth nth buyable percent)) list offset = 
    updateByPredicate list (nth-1) offset (targetMatches buyable) percent

procElem _ list offset = list

mapCart :: [Cart] -> [Cart]
mapCart list = mappedCart list
    where 
        iterateElements (count, cart) element = 
            (count + 1, procElem element cart count) 

        mappedCart = snd . foldl(iterateElements) (0, list)

