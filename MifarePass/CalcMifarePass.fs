//----------------------------------------------------------------------------
//
// Copyright (c) 2011 Nodir Gulyamov <gelvaos at gmail.com>. 
//
// This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// By using this source code in any fashion, you are agreeing to be bound 
// by the terms of the Apache License, Version 2.0.
//
// You must not remove this notice, or any other, from this software.
//----------------------------------------------------------------------------

module MifarePassword

open System
open System.IO
open System.Security.Cryptography

let hexStr2BytesList (hex_str : string) =
    let len = hex_str.Length
    if len % 2 <> 0 then
        failwith "Illegal string %s length" hex_str

    [ for start in 0..2..(len - 2) do yield System.Convert.ToByte (hex_str.[start..(start + 1)], 16) ]

let invert (buf : byte[]) =
    let len = buf.Length
    let invbuf : byte[] = Array.zeroCreate len
    for i in 0..(len-1) do invbuf.[len-i-1] <- buf.[i]
    invbuf

let bytesArr2HexString (arr : byte[]) =
    (BitConverter.ToString(arr)).Replace("-", "")

let calcMifarePassword (key_a_str, key_b_str) =
    let key_a = hexStr2BytesList key_a_str
    let key_b = hexStr2BytesList key_b_str

    let d_key_a : byte[] = Array.zeroCreate 8
    let d_key_b : byte[] = Array.zeroCreate 8
    for i in 0..5 do
        d_key_a.[i]   <-  key_a.[i] <<< 1
        d_key_b.[i+2] <-  key_b.[i] <<< 1

    for i in 0..5 do
        d_key_a.[6] <- d_key_a.[6] ||| ((((List.nth key_a (i)) &&& 255uy ) >>> 7) <<< (6-i))
        d_key_b.[1] <- d_key_b.[1] ||| ((((List.nth key_b (5-i)) &&& 255uy ) >>> 7) <<< (6-i))

    let tdes_key = Array.append (Array.append (invert d_key_a) (invert d_key_b)) (invert d_key_a)
    let start_data : byte[] = Array.zeroCreate 8
    let tdes = new TripleDESCryptoServiceProvider()
    tdes.Mode <- CipherMode.ECB
    tdes.Padding <- PaddingMode.None
    tdes.Key <- tdes_key
    
    let ic = tdes.CreateEncryptor ()
    let pass = ic.TransformFinalBlock(start_data, 0, 8)
    
    ( bytesArr2HexString d_key_a
      , bytesArr2HexString d_key_b
      , bytesArr2HexString (invert pass) )
    








