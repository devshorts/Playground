{-# OPTIONS_GHC -F -pgmF htfpp #-}
module TestRockpaper where

import Test.HUnit.Lang
import Test.Framework

import Rockpaper

-- this should be true
test_initial = assertEqual True (game Rock Scissors)

