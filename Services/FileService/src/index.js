const express = require('express');
var cors = require('cors')

require('dotenv').config()

const app = express();


var corsOptions = {
    origin: ['http://localhost:8081', 'http://admin.kltn.com'],
    optionsSuccessStatus: 200
  }
app.use(cors(corsOptions))

app.use(express.json());
app.use(express.urlencoded({
  extended: true
}));

const port = 3000;
const Minio = require("minio"); 



var minioClient = new Minio.Client({
    endPoint: 'localhost',
    port: 9000,
    useSSL: false,
    accessKey: process.env.MINIO_ACCESSKEY,
    secretKey: process.env.MINIO_SECRETKEY,
  })

app.get('/upload', async (req, res) => {
    minioClient.fPutObject('uet-kltn', 'file.png', './file.png', {}, (err, etag)  => {
        if(err) {
            return console.log(err)
        } 
        console.log(etag);
    })
    res.send('Hello World!');
});

app.get('/test', async (req, res) => {
    res.send('Hello World!');
})

app.post('/check-connection', async (req, res) => {
    const body = req.body;
    const client = new Minio.Client({
        endPoint: body.Endpoint,
        port: body.Port,
        useSSL: false,
        accessKey: body.AccessKey,
        secretKey: body.SecretKey,
    });


    if (body.AutoCreateMinio) {
        client.listBuckets(function(err, buckets) {
            if (err) {
                console.log(err);
                res.status(200).send(false);
            } else {
                res.status(200).send(true);
            }
        });

    } else {
        client.bucketExists(body.BucketName, function(err, exists) {
            if (err || !exists) {
                console.log(err);
                res.status(200).send(false);
            } else {
                res.status(200).send(true);
            }
        });
    }

    return false;

});

app.post('/active-tenant', async (req, res) => {
    const body = req.body;
    console.log(req.body);
    if (body.AutoCreateMinio) {
        minioClient.makeBucket(body.MinioBucketName, 'us-east-1', function(err) {
            if (err) {
                console.log(err);
                res.status(200).send(false);
            } else {
                res.status(200).send(true);
            }
        });
    } else {
        return true;
    }
});

app.listen(port, () => {
    console.log(`Server is running at http://localhost:${port}`);
});