BlobBatchUpdate
===============

Azure storage upload tools like azcopy, always set blob default content type to application/octet-stream. If the blob files are used by website or CDN, and is one of following types - html/css/xml/jpg/png/gif/video and etc, they will be handled wrongly.

BlobBatchUpdate can batch correct blob file content-type automatically based on file extension. For example, instead of having every blob contect type set to application/octet-stream, BlobBatchUpdate will set jpg file contect type to image/jpeg and css file content type to text/css, and so on ...
