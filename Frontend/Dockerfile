# Build stage: Node installeert dependencies en voert de Vite-build uit.
FROM node:24-alpine AS build
WORKDIR /app

# Kopieer eerst de packagebestanden voor een herbruikbare npm-ci-cachelaag.
COPY package.json package-lock.json ./
RUN npm ci

# Kopieer daarna de frontendbroncode.
COPY . .

# Vite verwerkt deze URL tijdens de build in de JavaScript-bundle.
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build

# Runtime stage: Nginx serveert alleen de statische productiebuild.
FROM nginx:alpine AS final

# Gebruik de configuratie met de index.html-fallback voor React Router.
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Neem alleen de gegenereerde dist-map uit de build stage over.
COPY --from=build /app/dist /usr/share/nginx/html

# Nginx luistert binnen de container op poort 80.
EXPOSE 80