name 'Relativity'
maintainer 'The Authors'
maintainer_email 'you@example.com'
license 'All Rights Reserved'
description 'Installs/Configures relativity'
long_description 'Installs/Configures relativity'
version '0.1.0'
chef_version '>= 12.1' if respond_to?(:chef_version)

depends 'vs_code', '~> 0.1.0'
depends 'powershell', '= 6.0.0'
depends 'chocolatey', '~> 1.2.0'
depends 'webpi', '~> 4.2.0'
