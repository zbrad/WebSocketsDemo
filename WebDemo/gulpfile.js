/// <binding BeforeBuild='tsc' Clean='clean' />

var G = {
    project: require("./project.json"),
    node: require("./package.json")
};

for (var d in G.node.devDependencies) {
    var name = d.split("-").pop();
    G[name] = require(d);
}

var paths = {
  bower: "./bower_components/",
  lib: "./" + G.project.webroot + "/lib/",
  ts: [
        "**/**.ts",
        "!./node_modules/**"
        ] 
};

G.gulp.task("clean", function (cb) {
  G.del(paths.lib, cb);
});

G.gulp.task("copy", ["clean"], function () {
    var bower = {
        "angular": "angular/angular*.{js,map}",
        "angular-route": "angular-route/angular-route*.{js,map}",
        "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
        "bootstrap-touch-carousel": "bootstrap-touch-carousel/dist/**/*.{js,css}",
        "hammer.js": "hammer.js/**/*.{js,map}",
        "jquery": "jquery/dist/**/*.{js,map}",
        "jquery-validation": "jquery-validation/dist/**/*.{js,map}",
        "jquery-validation-unobtrusive": "jquery-validation-unobtrusive/*.{js,map}"
    };

    for (var d in bower) {
        G.gulp.src(paths.bower + bower[d])
          .pipe(G.gulp.dest(paths.lib + d));
    }
});

G.gulp.task("tsc", ["copy"], function (cb) {
    return G.run("tsc").exec(cb);
});


