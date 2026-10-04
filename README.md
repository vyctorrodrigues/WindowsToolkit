# Windows Toolkit

Aplicação desktop desenvolvida em **C# e WPF** para centralizar ferramentas de suporte e manutenção de computadores Windows em uma única interface.

O projeto foi desenvolvido a partir de situações práticas encontradas no dia a dia de **suporte técnico**, buscando reduzir tarefas repetitivas e facilitar a execução de procedimentos de manutenção.

## Tecnologias

* C#
* .NET
* WPF
* XAML
* Windows API
* Git / GitHub

## Funcionalidades

### 🌐 Rede

* Consulta de informações dos adaptadores de rede
* Operações de manutenção e diagnóstico de rede
* Gerenciamento de configurações relacionadas à conectividade

### 🖨️ Impressoras

* Consulta de impressoras instaladas
* Gerenciamento de impressoras
* Adição de impressoras
* Gerenciamento do serviço de spooler

### 🧹 Limpeza

* Ferramentas para limpeza de arquivos temporários
* Rotinas de manutenção do sistema

### 💻 Sistema

* Consulta de informações do computador
* Informações de discos
* Operações de manutenção do Windows
* Ferramentas relacionadas ao sistema

### 🔐 Conta e acesso

* Alteração de senha
* Funcionalidades relacionadas ao login do usuário

### 🏢 Office

* Ferramentas relacionadas ao ambiente Microsoft Office

### 🎨 Interface

* Seleção de tema
* Seleção de cores
* Interface gráfica baseada em WPF

## Estrutura do projeto

```text
WindowsToolkit/
├── Models/
│   ├── AdaptadorRedeInfo.cs
│   ├── DiscoInfo.cs
│   ├── ImpressoraInfo.cs
│   ├── Licenca.cs
│   └── SistemaInfo.cs
│
├── Services/
│   ├── ImpressoraService.cs
│   ├── LicencaService.cs
│   ├── LimpezaService.cs
│   ├── LoginService.cs
│   ├── RedeService.cs
│   ├── ResultadoComando.cs
│   ├── SistemaService.cs
│   ├── SpoolerService.cs
│   └── TemaService.cs
│
├── Views/
│   ├── AdicionarImpressoraWindow.xaml
│   ├── AlterarSenhaWindow.xaml
│   ├── ImpressorasPage.xaml
│   ├── LimpezaPage.xaml
│   ├── LoginWindow.xaml
│   ├── MapearLptWindow.xaml
│   ├── OfficePage.xaml
│   ├── RedePage.xaml
│   ├── SelecionarCorWindow.xaml
│   ├── SelecionarTemaWindow.xaml
│   ├── SistemasPage.xaml
│   └── SobrePage.xaml
│
├── App.xaml
├── MainWindow.xaml
└── WindowsToolkit.csproj
```

## Objetivo

O Windows Toolkit tem como objetivo reunir em uma aplicação única ferramentas que podem ser utilizadas durante atividades de suporte técnico, evitando a necessidade de executar diversos procedimentos manualmente através de diferentes ferramentas do Windows.

Além da aplicação prática, o projeto também serve como estudo de **C#, WPF, desenvolvimento desktop, organização de código e integração com recursos do sistema operacional Windows**.

## Como executar

### Pré-requisitos

* Windows
* .NET SDK compatível com o projeto
* Visual Studio ou outra IDE compatível com projetos .NET/WPF

### Execução

Clone o repositório:

```bash
git clone https://github.com/vyctorrodrigues/WindowsToolkit.git
```

Entre na pasta do projeto e execute:

```bash
dotnet run --project WindowsToolkit
```

Algumas funcionalidades podem exigir **permissões administrativas**, dependendo da operação executada.

## Status

🚧 Projeto em desenvolvimento.

Novas ferramentas e melhorias de interface podem ser adicionadas conforme a necessidade.

## Autor

**Vyctor Rodrigues**

Projeto desenvolvido como parte do meu portfólio profissional na área de **Suporte Técnico e Desenvolvimento de Software**.
