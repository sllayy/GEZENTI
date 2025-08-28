# GEZENTI ---
  
Backend, [OSRM](http://project-osrm.org) motoruna bağlanarak rota hesaplama yapar.

---

## Özellikler

- **Driving / Foot modları** desteklenir.
- OptimizeOrder=true → `/trip` ile sıralama optimizasyonu.
- Alternatif rotalar (fastest / shortest / balanced seçim).
- GeoJSON geometri desteği.

## Gereksinimler

- [Docker](https://www.docker.com/) (OSRM servislerini çalıştırmak için)

---

## Kurulum

cd docker
docker compose up -d



Bu komut şunları başlatır:

osrm-driving (driving profile, port 5000)

osrm-foot (foot profile, port 5002)

osrm-gateway (nginx) (port 8081)

---
