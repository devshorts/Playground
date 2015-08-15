{-# LANGUAGE DeriveGeneric #-}

module Types where

import Data.Aeson
import GHC.Generics

data Person =
  Person  { firstName :: String
          ,lastName :: String
          } deriving(Show, Generic)

instance ToJSON Person
instance FromJSON Person
