/// <binding BeforeBuild='tsc' Clean='clean' />

var G = {
    project: require("./project.json"),
    node: require("./package.json"),
    tsconfig: require("./tsconfig.json")
};

for (var d in G.node.devDependencies) {
    var name = d.split("-").pop();
    G[name] = require(d);
}

var gulp = G.gulp;

var paths = {
    lib: "./" + G.project.webroot + "/lib/",
    css: "./" + G.project.webroot + "/lib/css/",
    scripts: "./" + G.project.webroot + "/lib/scripts/"
};

gulp.task("clean", function (cb) {
  G.del(paths.lib, cb);
});

gulp.task("copy-bower-min", [ 'clean' ], function () {
    return gulp.src([
        "./bower_components/**/*.min.js",
        "./bower_components/**/*.min.css"
    ], { base: "./bower_components/" }).pipe(gulp.dest(paths.lib));
});

gulp.task("copy-bower", ['clean'], function () {
    var wd = G.wiredep();
    var all = wd.css.concat(wd.js);
    return gulp.src(all, { base: "./bower_components" }).pipe(gulp.dest(paths.lib));
});

gulp.task("copy-scripts", ['clean','tsc'], function () {
    return gulp.src(["./client/**/*.js", "./common/**/*.js"], { base: "." })
        .pipe(gulp.dest(paths.scripts));
});

gulp.task("copy-css", ['clean'], function () {
    return gulp.src(["./client/**/*.css", "./common/**/*.css"], { base: "." })
        .pipe(gulp.dest(paths.css));
});

gulp.task("copy", [ "copy-bower", "copy-bower-min", "copy-scripts" , "copy-css"], function () {
    // xyz
});

gulp.task("tsc", function (cb) {
    var exe = "C:\\Program Files (x86)\\Microsoft SDKs\\TypeScript\\1.5\\tsc.exe";
    return G.run(exe).exec(cb);
});

function injectBower() {
    var options = {
        directory: "./bower_components",
        ignorePath: "../../../bower_components/",
        fileTypes: {
            cshtml: {
                block: /(([ \t]*)<!--\s*bower:*(\S*)\s*-->)(\n|\r|.)*?(<!--\s*endbower\s*-->)/gi,
                detect: {
                    js: /<script.*src=['"]([^'"]+)/gi,
                    css: /<link.*href=['"]([^'"]+)/gi
                },
                replace: {
                    js: '<script src="lib/{{filePath}}"></script>',
                    css: '<link rel="stylesheet" href="lib/{{filePath}}" />'
                }
            }
        }
    };

    return G.wiredep.stream(options);
};

function injectJs() {
    return G.inject(
            gulp.src([
                        './client/**/*.js', './common/**/*.js'
            ], { base: "." })
                .pipe(G.filesort()),
            {
                addRootSlash: false,
                transform: function (f) { return '<script src="lib/scripts/' + f + '"></script>'; }
            });
};

function injectCss() {
    return G.inject(
            gulp.src([
                './client/**/*.css', './common/**/*.css',
            ], { read: false, base: "." }),
            {
                addRootSlash: false,
                transform: function (f) { return '<link rel="stylesheet" href="lib/css/' + f + '" />'; }
            });
};


gulp.task('update-layout', ['tsc'], function () {
    var file = "server/Views/Shared/_Layout.cshtml";
    return gulp.src(file, { base: "." })
        .pipe(injectBower())
        .pipe(injectJs())
        .pipe(injectCss())
        .pipe(gulp.dest(paths.lib));
});

gulp.task("build", ['copy', 'update-layout'], function () {
    // xyz
});


gulp.task('test-wiredep', function () {
    var file = "./**/_Layout.cshtml";
    return gulp.src(file, { base: "." })
        .pipe(injectBower())
        .pipe(gulp.dest(paths.lib));
});

gulp.task('test-sort', function () {
    var file = "./**/_Layout.cshtml";
    return gulp.src(file, { base: "." })
        .pipe(G.inject(
            gulp.src([
                './client/**/*.js', './common/**/*.js'
            ], { base: "." })
                .pipe(G.filesort()
            ),
        {
            addRootSlash: false,
            transform: function (f) { return '<script src="lib/scripts/' + f + '"></script>'; }
        }))
        .pipe(gulp.dest(paths.lib));
});
