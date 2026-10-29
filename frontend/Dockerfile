# Build aşaması
FROM node:20-alpine AS build

WORKDIR /app

# Bağımlılık dosyalarını kopyala ve yükle
COPY package*.json ./
RUN npm ci

# Proje dosyalarını kopyala ve derle
COPY . .
RUN npm run build

# Production (yayınlama) aşaması
FROM nginx:stable-alpine AS production

# Derlenen dosyaları Nginx dizinine kopyala
COPY --from=build /app/dist /usr/share/nginx/html

# SPA routing (React Router vb.) desteği için temel Nginx yapılandırması
RUN printf 'server {\n\
    listen 80;\n\
    location / {\n\
        root /usr/share/nginx/html;\n\
        index index.html index.htm;\n\
        try_files $uri $uri/ /index.html;\n\
    }\n\
}\n' > /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

