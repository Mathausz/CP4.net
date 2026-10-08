# Evidências do CP5

- `headers-v1-query.txt`, `headers-v1-header.txt`, `headers-sem-versao.txt`: headers `api-supported-versions: 2.0` e `api-deprecated-versions: 1.0`
- `versao-nao-suportada-400.txt`: versão inexistente (9.9) responde 400
- `swagger-v1.0.json`, `swagger-v2.0.json`, `swagger-v1-descricao.txt`, `swagger-paths.txt`: Swagger por versão, v1 marcada como deprecada
- `swagger-grupos.png`: print do Swagger com os dois grupos (v1.0 DEPRECADA e v2.0)
- `400-page.txt`, `400-pagesize.txt`: 400 de page e pageSize inválidos (`application/problem+json`)
- `rate-limit-tentativas.txt`, `429-retry-after.txt`: estouro do POST /api/pedidos (429, Retry-After, corpo JSON)
- `health-apos-429.txt`: /health logo depois do 429 (nunca 429; 503 aqui porque o Oracle estava indisponível)
- `dotnet-test.txt`: saída do dotnet test
- `nota-oracle.txt`: explica que, com o Oracle indisponível, os GETs com dados (v1 array, v2 envelope, páginas 1 e 2, página vazia) não puderam ser capturados nesta rodada

Todos os arquivos foram gerados executando a API localmente.
