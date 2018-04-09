### <img src="http://cloud.google.com/_static/images/cloud/products/logos/svg/pubsub.svg" width=64> Sudokumb distributes tiny fragments of work via [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).

Distributing work across multiple machines is smart, but breaking up a tiny problem into trivial fragments and distributing them across multiple machines is dumb. The network overhead is orders of magnitude greater than just solving the problem in place.

However, creating lots of distributed work is a good demonstration of the power and scalability of [ Google Cloud Pub/Sub](https://cloud.google.com/pubsub/docs/).  Sudokumb can pump 100,000 messages per second through Pub/Sub
without a hiccup.
