module Common.Utils where

(|>) :: t -> (t -> b) -> b
(|>) a b = b $ a

(>>>) :: (a -> b) -> (b -> c) -> (a -> c)
(>>>) a b = b . a

