{-# OPTIONS_GHC -F -pgmF htfpp #-}
module Main where


import Test.Framework

--import {-@ HTF_TESTS @-} TestTemp
import {-@ HTF_TESTS @-} TestPrefixTree

main :: IO()
main = htfMain htf_importedTests  