# FreelanceManagerMVC

Sistema de gerenciamento para freelancers desenvolvido com ASP.NET Core, Blazor e PostgreSQL.

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop/)
- [Docker Compose](https://docs.docker.com/compose/install/) (geralmente já incluído com o Docker Desktop)

## Configuração inicial

### 1. Configuração do banco de dados com Docker

O projeto utiliza PostgreSQL como banco de dados. Para facilitar o processo de desenvolvimento, fornecemos um arquivo `docker-compose.yml` que configura automaticamente o PostgreSQL e o pgAdmin.

```bash
# Inicie os contêineres do PostgreSQL e pgAdmin
docker-compose up -d
```

Este comando iniciará:
- PostgreSQL na porta 5432
- pgAdmin na porta 5050 (acessível em http://localhost:5050)
  - Email: admin@admin.com
  - Senha: admin

### 2. Configuração do arquivo .env

Crie um arquivo `.env` baseado no `.env.example` para configurar a chave de licença do Syncfusion:

```bash
# Copie o arquivo .env.example
cp .env.example .env
```

### 3. Restaurar pacotes NuGet

```bash
dotnet restore
```

### 4. Aplicar migrações ao banco de dados

```bash
# Instalar a ferramenta Entity Framework Core CLI (se ainda não estiver instalada)
dotnet tool install --global dotnet-ef

# Aplicar migrações ao banco de dados
dotnet ef database update
```

## Executando o projeto

### Modo normal

```bash
dotnet run
```

### Modo de desenvolvimento com hot reload

Para desenvolvimento, é recomendado usar o modo "watch", que recompila e reinicia automaticamente a aplicação sempre que você faz alterações no código:

```bash
dotnet watch run
```

Com o hot reload ativado, você pode:
- Editar arquivos Razor (.razor) e ver as alterações imediatamente
- Pressionar "Ctrl + R" para reiniciar a aplicação em caso de alterações mais profundas

## Acessando a aplicação

- **Aplicação Web**: http://localhost:5064 (HTTP) ou https://localhost:7089 (HTTPS)
- **pgAdmin**: http://localhost:5050
  - Email: admin@admin.com
  - Senha: admin

## Estrutura do projeto

- **Components**: Componentes Blazor e páginas da aplicação
- **Data**: Modelos de dados e contexto do Entity Framework
- **Services**: Serviços de negócios para clientes, projetos e tarefas
- **IO**: Objetos de entrada/saída para transferência de dados
- **Migrations**: Migrações do banco de dados

## Parando os serviços

```bash
# Parar a aplicação ASP.NET Core
Ctrl+C no terminal onde a aplicação está rodando

# Parar os contêineres Docker
docker-compose down
```

## Limpeza completa (incluindo volumes)

```bash
# Para remover completamente os contêineres e os volumes (CUIDADO: isso apagará todos os dados do banco)
docker-compose down -v
```
