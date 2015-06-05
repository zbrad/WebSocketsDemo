/// <binding BeforeBuild='default' Clean='clean' ProjectOpened='watch' />

'use strict';

var gulp = require('gulp');
var inject = require('gulp-inject');
var run = require('gulp-run');
var del = require('del');
var fs = require('fs');
var config = require('./gulpconfig.json');

/**
 * Generates the {allRefs}.d.ts references file dynamically from all application *.ts files.
 */
gulp.task('gen-refs', function () {
    var allRef = config.typings + "/" + config.allRef;

    var text = '//{\n//}\n';
    fs.writeFileSync(allRef, text);

    var sources = gulp.src(config.source + "/**/**.ts", { read: false });
    var injected = inject(sources,
        {
            starttag: '//{',
            endtag: '//}',
            transform: function (filepath) {
                return '/// <reference path="..' + filepath + '" />';
            }
        });

    var target = gulp.src(allRef);
    var dest = gulp.dest(config.typings);

    return target.pipe(injected).pipe(dest);
});

gulp.task('default', ['gen-refs'], function () {
    return run("tsc").exec();
});

/**
 * Remove all generated JavaScript files from TypeScript compilation.
 */
gulp.task('clean', function (cb) {
    var genFiles = [
        config.typings + "/" + config.allRef,
        config.outputPath,  // path to generated JS files
        config.outputPath + '/**/**.js',    // path to all JS files auto gen'd by editor
        config.outputPath + '/**/**.js.map' // path to all sourcemap files auto gen'd by editor
    ];

    // delete the files
    del(genFiles, cb);
});

gulp.task('watch', function () {
    gulp.watch([config.source + "/**/**.ts"], ['gen-refs', 'compile']);
});
