import ListUtils
import Data.Aeson
import Data.Aeson.Encode.Pretty
import qualified Data.ByteString.Lazy as BSL
import Data.Text
import Data.Text.Encoding
import Types
process :: IO String
process = getLine

getJson d = unpack $ decodeUtf8 $ BSL.toStrict (encodePretty d)

main = do
  putStrLn "First name"
  firstName <- process

  putStrLn "Last name"
  lastName <- process

  let person = Person firstName lastName

  putStr $ getJson person

  return ()
