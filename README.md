# bitplate
Een CMS in ASP.NET / jquery

Deze repository bestaat uit de volgende .NET projects:

* HJORM: een ORM gemaakt om de domein objecten op te staan in een database en er weer uit te halen. Momenteel alleen MySql database geimplementeerd
* Domain: domain model met alle CMS objecten zoals page, template, container en module
* BitSite: de CMS-webapplicatie waarin gebruikers hun sites kunnen maken
* Sites: hierin verschijnt per site een (gepubliceerde) folder die binnen IIS een eigen website wordt

Verder nog de ondersteunende projecten:
* BitMetaServer: voor het uitdelen van licenties aan klanten
* BitBackupServer: voor het maken van backups


