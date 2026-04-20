# ONGES.Users.Api

API REST para gerenciamento de usuários do sistema ONGES, desenvolvida com .NET 10 seguindo princípios de Clean Architecture e Domain-Driven Design (DDD).

## Tecnologias

- **.NET 10 / ASP.NET Core** — Framework da API
- **Entity Framework Core 10** — ORM com SQL Server
- **MongoDB** — Event Store para Event Sourcing
- **FluentValidation** — Validação de requests
- **JWT (JSON Web Token)** — Autenticação e autorização
- **xUnit + Moq** — Testes unitários
- **Docker** — Containerização da aplicação
- **Kubernetes** — Orquestração de containers
- **GitHub Actions** — CI/CD (build, test, push de imagem Docker)
- **Scalar** — Documentação interativa da API (ambiente de desenvolvimento)

## Padrões e Arquitetura

- **Clean Architecture** — Separação em camadas: Domain, Application, Infrastructure e Api
- **Domain-Driven Design (DDD)** — Entidades, Value Objects, Exceptions de domínio
- **Event Sourcing** — Registro de eventos de domínio no MongoDB
- **Repository Pattern** — Abstração de acesso a dados com repositório genérico
- **Result Pattern** — Retorno padronizado de operações sem uso de exceções para fluxo de controle
- **Minimal APIs** — Endpoints mapeados com organização modular

## Estrutura do Projeto

```
ONGES.Users.Api/          → Camada de apresentação (endpoints, middlewares)
ONGES.Users.Application/  → Contratos (interfaces, DTOs, eventos)
ONGES.Users.Domain/       → Regras de negócio (entidades, value objects, exceções)
ONGES.Users.Infrastructure/ → Implementações (repositórios, serviços, validadores, EF Core)
ONGES.Users.Test/         → Testes unitários (domínio, aplicação, infraestrutura)
k8s/                      → Manifests Kubernetes
```

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) com **Kubernetes habilitado**

## Como rodar localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/alexandresorza/ONGES.Users.Api.git
cd ONGES.Users.Api
```

### 2. Aplicar os manifests do Kubernetes

```bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secrets.yaml
kubectl apply -f k8s/services.yaml
kubectl apply -f k8s/mssql.yaml
kubectl apply -f k8s/mongo.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/hpa.yaml
```

### 3. Verificar se os pods estão rodando

```bash
kubectl get pods -n onges-users
```

Aguarde até que todos os pods estejam com status `Running` e `READY 1/1`.

### 4. Acessar a API

A API estará disponível em:

```
http://localhost/health
```

> O Service da API utiliza `LoadBalancer`, que no Docker Desktop mapeia automaticamente para `localhost` na porta 80.

### 5. Rodar os testes

```bash
dotnet test
```

## Endpoints

| Método | Rota | Descrição | Autenticação |
|--------|------|-----------|--------------|
| GET | `/health` | Health check | Não |
| POST | `/users` | Criar usuário | Não |
| POST | `/users/auth` | Autenticar | Não |
| GET | `/users/auth-check` | Verificar autenticação | Sim (JWT) |
| GET | `/users` | Listar todos | Sim (Gestor) |
| GET | `/users/{id}` | Buscar por ID | Não |
| DELETE | `/users/{id}` | Remover usuário | Sim (Gestor) |
| DELETE | `/users/deactivate/{id}` | Desativar | Sim (Gestor) |
| PUT | `/users/activate/{id}` | Ativar | Sim (Gestor) |
| PUT | `/users/role` | Alterar perfil | Sim (Gestor) |