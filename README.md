
# Operatus Web API

Este é um projeto de **API** RESTful utilizando **ASP.NET Core**, implementando o **Repository Pattern** para o gerenciamento do Operatus, utilizando o banco de dados **SQL Server**.

## Estrutura do Projeto

A estrutura do projeto segue uma organização baseada no **Repository Pattern**, separando as camadas de **acesso a dados**, **serviços** e **controllers**.

### Estrutura de Pastas

```
/FSBR-WebAPI
│
├── /Controllers               # Camada de controle, onde a API é exposta.
│   ├── 
│   ├── 
│
├── /Entities                  # Contém os modelos de dados
│   ├── 
│   ├── 
│
├── /Domain                    # Camada de serviço, contendo a lógica de negócios.
│   ├── /Interfaces            # Interfaces dos repositórios e serviços.
│   │   ├── 
│   │   ├── 
│   │
│   ├── /Services              # Implementações dos serviços.
│   │   ├── 
│   │   ├── 
│
├── /Infrastructure            # Camada de acesso a dados e repositórios.
│   ├── /Configuration         # Configuração do DbContext e dependências.
│   │   ├── ContextBase.cs     # DbContext para o acesso ao banco de dados.
│   │
│   ├── /Repository            # Implementação dos repositórios.
│   │   ├── 
│   │   ├── 
│
├── /Migrations                # Migrations do Entity Framework para criação do banco de dados.
│
├── /appsettings.json          # Arquivo de configuração com strings de conexão e outras variáveis.
├── /Program.cs                # Configuração da aplicação e serviços (DI).
├── /Startup.cs                # Configuração da aplicação e serviços (para versões anteriores).
```

## Como Rodar o Projeto

### **1. Instalar Dependências**

Certifique-se de que as dependências do projeto estejam instaladas. Você pode rodar o comando:

```bash
dotnet restore
```

### **2. Criar o Banco de Dados**

Se ainda não foi criado, você pode gerar as **migrations** e aplicar no **LocalDB** executando:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### **3. Rodar o Projeto**

Execute o projeto com o comando:

```bash
dotnet run
```

A API estará disponível em **http://localhost:5000** (por padrão, ou conforme configurado).
A API estará disponível em **http://localhost:5000/swagger** (por padrão, ou conforme configurado).


### **Licença**

Este projeto está licenciado sob a **MIT License** - consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
