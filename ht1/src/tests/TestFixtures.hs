{-# OPTIONS_GHC -F -pgmF htfpp #-}
module TestFixtures where

import Test.HUnit.Lang
import Test.Framework

import Foo

defaultCart =  [Product Fruit $ Price 2.0, 
            Discount $ All 0.5,
            Discount $ Nth 1 Meat 0.5,
            Product Meat $ Price 1.0,
            Product Meat $ Price 2.0]

test_targetMatches = assertEqual True buy
    where buy = targetMatches Meat (Product Meat $ Price 2.0)

test_isBuyableFalse = assertEqual False buy
    where buy = targetMatches Meat (Product Fruit $ Price 2.0)      

test_updateCart = assertEqual expected updatedItem
    where         
        item = Product Fruit $ Price 1.0
        expected = Product Fruit $ Price 0.5
        updatedItem = updateCart 0.5 item

test_mapAllItems = assertEqual target actual
    where
        original = [Product Fruit $ Price 2.0, 
                    Discount $ All 0.5,
                    Discount $ Nth 1 Meat 0.5,
                    Product Meat $ Price 2.0,
                    Product Meat $ Price 2.0]

        target = [  Product Fruit $ Price 1.0, 
                    Discount $ All 0.5,
                    Discount $ Nth 1 Meat 0.5,
                    Product Meat $ Price 0.5,
                    Product Meat $ Price 1.0]

        actual = mapCart original

test_mapAllItems2 = assertEqual target actual
    where
        original = [Product Fruit $ Price 2.0, 
                    Discount $ All 0.5,
                    Discount $ Nth 2 Meat 0.5,
                    Product Meat $ Price 1.0,
                    Product Meat $ Price 2.0]

        target = [  Product Fruit $ Price 1.0, 
                    Discount $ All 0.5,
                    Discount $ Nth 2 Meat 0.5,
                    Product Meat $ Price 0.5,
                    Product Meat $ Price 0.5]

        actual = mapCart original

test_getNth_at_element = assertEqual (Just 3) actual            
    where 
        actual = getNth 1 0 isProduct defaultCart

test_getNth_one_element_later = assertEqual (Just 4) actual            
    where 
        actual = getNth 1 1 isProduct defaultCart