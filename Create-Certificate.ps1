CD ./certificates

# The next line may need Administrator access. It creates a local CA.
mkcert -install

# These lines copy the local CA cert to the ./certificates folder
copy $env:LOCALAPPDATA\mkcert\rootCA-key.pem ./cacerts.pem
copy $env:LOCALAPPDATA\mkcert\rootCA.pem ./cacerts.crt

# This line creates a wildcart certificate based on the local CA.
mkcert -cert-file acme.local.crt -key-file acme.local.key acme.local *.acme.local