/// <binding BeforeBuild='gen-refs, default' />
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
gulp.task('update-refs', function () {

    // find files
    var sources = gulp.src(config.source + "/**/**.ts", { read: false });

    // create ref list
    var injectRefList = inject(sources,
        {
            relative: true,
            starttag: '//{',
            endtag: '//}',
            transform: function (filepath) {
                return '/// <reference path="' + filepath + '" />';
            }
        });

    // generate refs
    var allRef = config.typings + "/" + config.allRef;
    var text = '//{\n//}\n';
    fs.writeFileSync(allRef, text);
    var targetRef = gulp.src(allRef);
    var genRef = targetRef.pipe(injectRefList).pipe(gulp.dest(config.typings));

    return genRef;
});

gulp.task('update-tsconfig', function () {

    // find files
    var sources = gulp.src(config.source + "/**/**.ts", { read: false });

    // create file list
    var injectFileList = inject(sources,
    {
        relative: true,
        starttag: '"files": [',
        endtag: ']',
        transform: function (filepath, file, i, length) {
            return '"' + filepath + '"' + (i + 1 < length ? ',' : '');
        }
    });

    // generate file list
    var targetTsc = gulp.src('./tsconfig.json');
    var genTsc = targetTsc.pipe(injectFileList).pipe(gulp.dest('.'));

    return genTsc;
});

gulp.task('default', ['clean', 'update-refs', 'update-tsconfig'], function () {
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
    gulp.watch([config.source + "/**/**.ts"], ['update-refs', 'update-tsconfig', 'compile']);
});
