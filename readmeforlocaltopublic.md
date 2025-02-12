https://ngrok.com/docs/agent/

UP amp143..Ke7

choco install ngrok
ngrok config add-authtoken <token>
ngrok http 80


ngrok config add-authtoken 2s4BsTKB9FcuSKirg2caumjS4ra_7J4N6ofpp6ksYKXhtGacN
Authtoken saved to configuration file: C:\Users\amit.p\AppData\Local/ngrok/ngrok.yml


ngrok http http://localhost:8081/

ngrok http 8081 --basic-auth "xyz:Xyz@1234" --basic-auth "abc:Abc@1234"

ngrok http http://localhost:8081/ --basic-auth "xyz:Xyz@1234" --basic-auth "abc:Abc@1234"

http://127.0.0.1:4040

## Other Tool

https://theboroer.github.io/localtunnel-www/