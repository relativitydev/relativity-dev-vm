name 'BaseImage'
maintainer 'The Authors'
maintainer_email 'you@example.com'
license 'All Rights Reserved'
description 'Installs/Configures BaseImage'
long_description 'Installs/Configures BaseImage'
version '0.1.0'
chef_version '>= 12.1' if respond_to?(:chef_version)

depends 'powershell', '= 6.0.0'
depends 'webpi', '~> 4.2.0'
depends 'msoffice', '~> 0.1.0'

# The `issues_url` points to the location where issues for this cookbook are
# tracked.  A `View Issues` link will be displayed on this cookbook's page when
# uploaded to a Supermarket.
#
# issues_url 'https://github.com/<insert_org_here>/BaseImage/issues'

# The `source_url` points to the development repository for this cookbook.  A
# `View Source` link will be displayed on this cookbook's page when uploaded to
# a Supermarket.
#
# source_url 'https://github.com/<insert_org_here>/BaseImage'
