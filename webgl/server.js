const express = require('express');
const app = express();
const http = require('http');
const path = require('path');
const port = 3000;
const fs = require('fs');
const WebSocket = require('ws');
var mysql = require('mysql');
var compression = require('compression');


app.use(compression());

// Use the whole root as static files to be able to serve the html file and
// the build folder
app.use(express.static(path.join(__dirname, '/'), {

    setHeaders: function (res, path) {
        if (path.endsWith(".unityweb")) {
            res.set("Content-Encoding", "br");
        }
    }

}));
// Send html on '/'path
app.get('/', (req, res) => {

    //res.set({'Content - Encoding':  "br"});


    res.header('Content-Encoding', 'br');
    //res.header('Content-Encoding', 'gz');
    res.sendFile(path.join(__dirname, + '/index.html'));
})


// Create the server and listen on port
server = http.createServer(app);
//.listen(port, () => {
// console.log(`Server running on localhost:${port}`);
//});




const wss = new WebSocket.Server({ server });

server.listen(port, () => {
    console.log(`Server running on localhost:${port}`);
});

wss.on('connection', ((ws) => {

    var con = mysql.createConnection({
        host: "127.0.0.1",
        user: "root",
        password: "Motion12!"
    });

    con.connect(function (err) {
        console.log("Connected to web socket");

        if (err) throw err;

        con.query("CREATE DATABASE IF NOT EXISTS  mobsDocileDB2", function (err, result) {
            if (err) throw err;
            console.log("Database created");
        });

        con.query("USE mobsDocileDB2", function (err, result) {
            if (err) throw err;
            console.log("Database created");
        });


        var sql = "CREATE TABLE IF NOT EXISTS  users (id VARCHAR(255), age INT, gender TEXT, nationality TEXT, familiarity INT, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });



        var sql = "CREATE TABLE IF NOT EXISTS  userStats (id VARCHAR(255), timeSpent FLOAT, fightCnt INT, punchCnt INT, avgSpeed FLOAT, totalDist FLOAT, collectedItemCnt INT, totalItemCnt INT, crowdPersonality INT, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });

        var sql = "CREATE TABLE IF NOT EXISTS  preSurveyResponses (id VARCHAR(255), responses TEXT, crowdPersonality INT, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });

        var sql = "CREATE TABLE IF NOT EXISTS  postSurveyResponses (id VARCHAR(255), responses TEXT, crowdPersonality INT, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });

        var sql = "CREATE TABLE IF NOT EXISTS  personalityResponses (id VARCHAR(255), responses TEXT, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });


    });

    ws.on('message', (message) => {
        if (message != '') {

            obj = JSON.parse(message);

            if (obj.type == "demographics") {
                var sql = "INSERT IGNORE INTO users (id, age, gender, nationality, familiarity) VALUES( \"" + obj.id + "\" ," + obj.age + ", \" " + obj.gender + " \" ,\" " + obj.nationality + " \" , " + obj.familiarity +")";
                con.query(sql, function (err, result) {
                    if (err) throw err;
                    console.log("1 record inserted");
                    console.log(result);
                });
            }

            else if (obj.type == "personality") {

                var sql = "INSERT IGNORE INTO personalityResponses (id, responses) VALUES( \"" + obj.id + "\" ,\" " + obj.responses + "\" )";
                con.query(sql, function (err, result) {
                    if (err) throw err;
                    console.log(obj.responses);
                    console.log("1 record for personality inserted");
                    console.log(result);
                });
            }
            else if (obj.type == "preStudy") {
                var sql = "INSERT IGNORE INTO preSurveyResponses (id, responses, crowdPersonality) VALUES( \"" + obj.id + "\" ,\" " + obj.responses + "\" , " + obj.crowdPersonality+ "\)";
                con.query(sql, function (err, result) {
                    if (err) throw err;
                    console.log("1 record inserted");
                    console.log(result);
                });
            }
            else if (obj.type == "postStudy") {
                var sql = "INSERT IGNORE INTO postSurveyResponses (id, responses, crowdPersonality) VALUES( \"" + obj.id + "\" ,\" " + obj.responses + "\" , " + obj.crowdPersonality + "\)";
                con.query(sql, function (err, result) {
                    if (err) throw err;
                    console.log("1 record inserted");
                    console.log(result);
                });
            }
            else if (obj.type == "stats") {
                var sql = "INSERT IGNORE INTO userStats (id, timeSpent, fightCnt, punchCnt, avgSpeed, totalDist, collectedItemCnt, totalItemCnt, crowdPersonality) VALUES( \"" + obj.id + "\" ," + obj.timeSpent + ",  " + obj.fightCnt + ",  " + obj.punchCnt + ",  "
                    + obj.avgSpeed + " , " + obj.totalDist + " , " + obj.collectedItemCnt + " , " + obj.totalItemCnt + " , " + obj.crowdPersonality + "  )";
                con.query(sql, function (err, result) {
                    if (err) throw err;
                    console.log("1 record inserted");
                    console.log(result);
                });
            }

        }

    });
    ws.on('end', () => {
        console.log('Connection ended...');
    });
    ws.send('Hello Client');
}));

