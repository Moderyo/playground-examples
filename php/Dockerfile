FROM php:8.3-cli
WORKDIR /app
RUN apt-get update && apt-get install -y unzip
COPY --from=composer:latest /usr/bin/composer /usr/bin/composer
COPY composer.json .
RUN composer install --no-dev
COPY . .
EXPOSE 4005
CMD ["php", "-S", "0.0.0.0:4005", "index.php"]
