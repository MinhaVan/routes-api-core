# Rodar no docker

docker build -t van-api .
docker run -d -p 5002:5001 -v ~/.aws:/root/.aws --name van-api-container van-api

# Subir para o EC2 (Apenas HTTP)

sudo dnf update -y
sudo dnf install -y dotnet-sdk-8.0
dotnet --version

sudo mkdir -p /var/www/van-api
sudo chown ec2-user:ec2-user /var/www/van-api

sudo nano /etc/systemd/system/van-api.service
`[Unit]
Description=Van API .NET 8
After=network.target

[Service]
WorkingDirectory=/var/www/van-api
ExecStart=/usr/bin/dotnet /var/www/van-api/YourApp.dll
Restart=always
User=ec2-user
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target`

sudo systemctl daemon-reload
sudo systemctl start van-api
sudo systemctl status van-api
sudo systemctl enable van-api

# Setando o HTTPs

Link da conversa: https://chatgpt.com/c/67b31e11-fd74-8006-a692-b005f42defda

sudo dnf update -y # Para Amazon Linux 2023
sudo dnf install -y nginx

sudo systemctl start nginx
sudo systemctl enable nginx

sudo systemctl status nginx

sudo dnf install -y certbot python3-certbot-nginx
sudo certbot --nginx -d api.cadeavan.com.br -d www.api.cadeavan.com.br

sudo nano /etc/nginx/conf.d/api.cadeavan.com.br.conf
`
server {
server_name api.cadeavan.com.br www.api.cadeavan.com.br;

    location / {
        proxy_pass http://localhost:5001;  # Certifique-se de que o backend esteja ouvindo na porta 5001
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /websocket {
        proxy_pass http://localhost:5001;  # Certifique-se que o SignalR está ouvindo nessa porta
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "Upgrade";
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    listen 443 ssl; # managed by Certbot
    ssl_certificate /etc/letsencrypt/live/api.cadeavan.com.br/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/api.cadeavan.com.br/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot

}
server {
if ($host = www.api.cadeavan.com.br) {
        return 301 https://$host$request_uri;
} # managed by Certbot

    if ($host = api.cadeavan.com.br) {
        return 301 https://$host$request_uri;
    } # managed by Certbot


    listen 80;
    server_name api.cadeavan.com.br www.api.cadeavan.com.br;
    return 301 https://$host$request_uri; # managed by Certbot

}
`
sudo nginx -t

sudo systemctl restart nginx

---ssss

key new relic: NRAK-U4EXWEJRX9RHUOBYTG8XEJMU43K
