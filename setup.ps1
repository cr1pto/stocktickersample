cinst mkcert -y
mkcert -pkcs12 localhost *.stockticker.local stocktickerapi stocktickerrazorpages 127.0.0.1 ::1
cp *.p12 $env:appdata\ASP.NET\Https\