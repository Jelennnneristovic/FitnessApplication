# Stage 1: Build Angular aplikacije
FROM node:20 AS build
WORKDIR /app

# Kopiraj package fajlove i instaliraj zavisnosti
COPY package.json package-lock.json ./
RUN npm install

# Kopiraj ostatak koda i build-uj
COPY . .
RUN npm run build

# Stage 2: Serviraj kroz Nginx
FROM nginx:alpine

# Kopiraj build rezultat u nginx html folder
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

# Kopiraj nginx konfiguraciju (za Angular routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80