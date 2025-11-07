# ⟢ Elysia API – DevOps

Este repositório contém a aplicação **Elysia** (.NET) integrada em um pipeline de **CI/CD** no **Azure DevOps**, com deploy automatizado em **Azure Web App (Linux/Container)**.

---

## ⚙️ Tecnologias / Stack
- .NET 9 + .NET 8 WebAPI
- EF Core + Npgsql
- Test xUnit
- PostgreSQL
- Docker
- Azure Container Registry (ACR)
- Azure App Service (Linux / Container)
- CI/CD

---

## Processo de CI (Build)

- Checkout do código
- dotnet restore, dotnet build, dotnet test
- Docker Build & Push para o ACR
- Artefato publicado para uso no Release
  
→ Sempre que ocorre **push na branch main**, o CI executa automaticamente..

## Processo de CD (Release)

- Artifact do Build CI
- Deploy em Azure Web App for Containers (Linux)
- Consumo da imagem elysia-api do ACR
- Atualização automática da imagem em produção
  
→ Após o CI finalizar, o CD pega a imagem recém gerada e **atualiza o ambiente de produção automaticamente**.

---

## Testando a API
⟢ Acesse o Swagger em: ``` link soon ``` </br>
⟢ azurewebsites endpoints: ``` link soon ```

---

# ⟢ Integrantes

➤ Iris Tavares Alves — 557728 </br>
➤ Taís Tavares Alves — 557553 
