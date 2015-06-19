Note project build is still in progress,
but I wanted to snapshot the current progress.

nuget packages:

        install-package AngularJS.Core
        install-package AngularJS.Route

global npm packages:

        npm install gulp         -g
        npm install tsd          -g

local npm packages:

        npm install gulp         --save-dev
        npm install del          --save-dev
        npm install gulp-inject  --save-dev
        npm install gulp-jshint  --save-dev
        npm install gulp-concat  --save-dev
        npm install gulp-uglify  --save-dev
        npm install gulp-rename  --save-dev
        npm install gulp-run     --save-dev
		npm install wiredep      --save-dev





setup typings:

        tsd init
        tsd install jquery
        tsd install angular

*New pet peve*: why does each project have to have it's own node_modules collection,
rather than a shared solution (or multi-project one).

For each project:

setup gulp:


