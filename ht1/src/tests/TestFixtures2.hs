{-# OPTIONS_GHC -F -pgmF htfpp #-}
module TestFixtures2 where

import Test.HUnit.Lang
import Test.Framework

import Foo

test_dummy = assertEqual True True