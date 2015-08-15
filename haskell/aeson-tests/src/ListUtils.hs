module ListUtils where

transform :: [a] -> Int
transform = length

pad :: [a] -> Int -> a -> [a]
pad list size padding =
  if takeSize > 0 then
    list ++ replicate takeSize padding
  else
    list
  where
    listSize = length list
    takeSize = size - listSize
