{-# OPTIONS_GHC -F -pgmF htfpp #-}
module Main where


import Test.Framework

import {-@ HTF_TESTS @-} TestFixtures
import {-@ HTF_TESTS @-} TestFixtures2

main :: IO()
main = htfMain htf_importedTests