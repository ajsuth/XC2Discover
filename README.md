# Sitecore Experience Commerce to Discover Feeds
This is sample plugin to convert Sitecore Commerce data into Sitecore Discover catalog feeds.

## Supported Sitecore Experience Commerce Versions
- XC 10.1

## Features

* Customer (Buyer) Migration
* Catalog Migration
* Category Migration
* Sellable Item (Product) Migration
* List Price Migration (Sellable Items only)
* Inventory Migration

## Preparing a Sitecore Discover Domain

There are a few pre-requisite tasks required in Sitecore Discover prior to being able to ingest the feeds generated from XC data. These include:
* Configure feed file names.
* Configure product model and industry vertical.
* Create custom product attributes.

## Running the Feed Generation

The postman collection can be imported from the solution's `/postman` folder. The **_Create Discover Feeds_** API uses the following manual configurations to control the importer functionality.

### Process Settings

The process settings controls the entites/objects that will be processed in the export.

#### Process Settings Example

```
"processSettings": {
    "@odata.type": "Ajsuth.Sample.Discover.Engine.Models.ExportSettings",
    "ProcessCategories": false,
    "ProcessProducts": false
}
```

#### Product Settings Properties

* **`ProcessCategories`:** If enabled, translates XC categories into a category feed.
* **`ProcessProducts`:** If enabled, translates XC sellable items into a product feed.

### Site Settings

The site settings is an array of Sitecore storefronts that utilise XC.

#### Site Settings Example

```
"siteSettings": [
    {
        "@odata.type": "Ajsuth.Sample.Discover.Engine.Policies.SitePolicy",
        "Name": "Storefront",
        "Catalog": "Habitat_Master",
        "Domain": "Storefront",
        "Storefront": "Storefront"
    }
]
```

#### Site Settings Properties

* **`Name`:** The name of the site. Found in the Sitecore Content Editor at *sitecore/content/content/\<tenant\>/\<site\>*.


![Site Name](/images/site-name.png)
* **`Catalog`:** The catalog friendly id associated to the site.


![Site Catalog](/images/site-catalog.png)
* **`Domain`:** The domain association to the site. Found in the Sitecore Content Editor at *sitecore/content/content/\<tenant\>/\<site\>/settings/sitegrouping/\<site group\>*.

![Site Domain](/images/site-domain.png)
* **`Storefront`:** The storefront associated to the site. Found in the Sitecore Content Editor at *sitecore/content/content/\<tenant\>/\<site\>/settings/commerce/controlpanelconfiguration*.

### Category Settings

The category settings manages behaviour of the category feed generator.

#### Category Settings Example

```
"categorySettings": {
    "@odata.type": "Ajsuth.Sample.Discover.Engine.Policies.CategoryFeedPolicy",
    "CategoryFeedFilePath": "C:\\Feeds\\Habitat_category_feed.csv"
}
```

#### Category Settings Properties

* **`CategoryFeedFilePath`:** The full file path, including file name, of the category feed to be created.

### Product Settings

The product settings manages behaviour of the product feed generator.

#### Product Settings Example

```
"productSettings": {
    "@odata.type": "Ajsuth.Sample.Discover.Engine.Policies.SellableItemFeedPolicy",
    "ProductFeedFilePath": "C:\\Feeds\\Habitat_product_feed.csv",
    "IncludeStandaloneProducts": true,
    "IncludeProductsWithVariants": false,
    "InventorySetId": "Habitat_Inventory",
    "DefaultCurrency":"USD",
    "IncludeImages": true
}
```

#### Product Settings Properties

* **`ProductFeedFilePath`:** The full file path, including file name, of the product feed to be created.
* **`IncludeStandaloneProducts`:** When true, sellable items will be converted to the Sitecore Discover product model. **Note:** Model 1 is currently not supported.
* **`IncludeProductsWithVariants`:** When true, sellable items will be converted to the Sitecore Discover product model. **Note:** Model 1 is currently not supported.
* **`InventorySetId`:** The inventory set to retrieve the inventory count from. **Note:** currently only supports single inventory.
* **`DefaultCurrency`:** The default currency is used to identify which price value to extract from the sellable item's list price.
* **`IncludeImages`:** If enabled, assumes product images are being served from Azure Storage and will construct urls expecting the image to exist.

### Cloud Storage Settings

The cloud storage settings will be eventually be moved into its own plugin later.

#### Cloud Storage Settings Example

```
"cloudStorageSettings": {
    "@odata.type": "Ajsuth.Sample.Discover.Engine.Policies.CloudStoragePolicy",
    "ConnectionString": "",
    "BaseUrl": "",
    "Container": ""
}
```

#### Cloud Storage Settings Properties

* **`ConnectionString`:** The connection string to the storage account.
* **`BaseUrl`:** The product image urls are generated using the following format, `<baseUrl><container>/<sellable item friendly id}/<image file name>`.
* **`Container`:** The product image urls are generated using the following format, `<baseUrl><container>/<sellable item friendly id}/<image file name>`.

## Installation Instructions
1. Download the repository.
2. Add the **Ajsuth.Sample.Discover.Engine.csproj** to the _**Sitecore Commerce Engine**_ solution.
3. In the _**Sitecore Commerce Engine**_ project, add a reference to the **Ajsuth.Sample.Discover.Engine** project.
4. Run the _**Sitecore Commerce Engine**_ from Visual Studio or deploy the solution and run from IIS.
5. Run the Bootstrap command on the _**Sitecore Commerce Engine**_.

## Known Issues
| Feature                 | Description | Issue |
| ----------------------- | ----------- | ----- |
|                         |             |       |

## Disclaimer
The code provided in this repository is sample code only. It is not intended for production usage and not endorsed by Sitecore.
Both Sitecore and the code author do not take responsibility for any issues caused as a result of using this code.
No guarantee or warranty is provided and code must be used at own risk.
