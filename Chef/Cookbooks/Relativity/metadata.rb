name 'Relativity'
maintainer 'The Authors'
maintainer_email 'you@example.com'
license 'All Rights Reserved'
description 'Installs/Configures relativity'
long_description 'Installs/Configures relativity'
version '0.1.0'
chef_version '>= 12.1' if respond_to?(:chef_version)

depends 'powershell', '= 6.0.0'
depends 'webpi', '~> 4.2.0'
depends 'msoffice', '~> 0.1.0'
depends 'seven_zip', '= 3.2.0'