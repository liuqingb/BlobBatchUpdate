BlobBatchUpdate
===============

A few Azure storage upload tools like azcopy, always set blob content type to application/octet-stream. If your blob files are used by website or CDN, you might find following files html/css/xml/jpg/png/gif/video and so on, not behaviour correctly due to incorrect content type.

BlobBatchUpdate can batch correct blob file content-type automatically based on file extension. For example, set blob files with jpg extension contect type to image/jpeg and css extension files content type to text/css.

Usage: BlobBatchUpdate -s StorageName -k StorageKey -c 0|1
        -s      Storage account name
        -k      Storage account key
        -c      [Optional]Use China Endpoint, default not use