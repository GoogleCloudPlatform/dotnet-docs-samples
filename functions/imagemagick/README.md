# ImageMagick sample

This sample has no automated test yet.
Eventually it should have a system test using two Storage buckets
("original" and "blurred") that:

- Deploys to Google Cloud Functions
- Uploads a zombie image to the "original" bucket
- Retrieves log entries to validate that the image was detected as
  inappropriate
- Checks for the existence of a matching file in the "blurred" bucket
- Uploads a cat image to the "original" bucket
- Retrives log entries to validate that the image was detected as "OK"
- Checks that no matching file was created in the "blurred" bucket

These steps have been checked manually, but this should be automated
eventually.
