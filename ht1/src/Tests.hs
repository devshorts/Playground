module Tests where

import Control.Applicative
import Text.Show.Functions

data Foo a = Thing a 


instance (Show a) => Show (Foo a) where
	show (Thing a) = show a

instance Functor Foo where
	fmap f (Thing a) = Thing $ f a

instance Applicative Foo where
	pure = Thing
	Thing f <*> Thing n = Thing $ f n

add3 :: Num a => a -> a -> a -> a
add3 a b c = a + b + c