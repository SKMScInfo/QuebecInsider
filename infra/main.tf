terraform {
  required_version = ">= 1.5.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
}

variable "location" {
  description = "Région Azure pour toutes les ressources"
  type        = string
  default     = "westus"
}

variable "resource_group_name" {
  description = "Nom du groupe de ressources Azure"
  type        = string
  default     = "rg-quebecinsider-dev"
}

variable "postgres_admin_password" {
  description = "Mot de passe administrateur PostgreSQL"
  type        = string
  sensitive   = true
}

variable "api_image" {
  description = "Image Docker du backend .NET 10 API"
  type        = string
  default     = "ghcr.io/skmscinfo/quebecinsider-api:latest"
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "law-quebecinsider"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "appi" {
  name                = "appi-2-quebecinsider"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  workspace_id        = azurerm_log_analytics_workspace.law.id
  application_type    = "web"
}

resource "azurerm_postgresql_flexible_server" "postgres" {
  name                   = "psql-quebecinsider-dev"
  resource_group_name    = azurerm_resource_group.rg.name
  location               = azurerm_resource_group.rg.location
  version                = "17"
  administrator_login    = "pgadmin"
  administrator_password = var.postgres_admin_password
  sku_name               = "B_Standard_B1ms"
  storage_mb             = 32768
}

resource "azurerm_container_app_environment" "cae" {
  name                       = "cae-quebecinsider"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}

resource "azurerm_container_app" "api" {
  name                         = "ca-quebecinsider-api"
  container_app_environment_id = azurerm_container_app_environment.cae.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  template {
    min_replicas = 0
    max_replicas = 2

    container {
      name   = "ca-quebecinsider-api"
      image  = var.api_image
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "APPLICATIONINSIGHTS_CONNECTION_STRING"
        value = azurerm_application_insights.appi.connection_string
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 8080
    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}

output "api_fqdn" {
  description = "URL publique de l'API Azure Container App"
  value       = azurerm_container_app.api.latest_revision_fqdn
}