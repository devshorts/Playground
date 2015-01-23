package com.devshorts.enumerable.iterators;

import java.util.function.Function;

public class MapIterator<TSource, TResult> extends EnumerableIterator<TResult> {

    private Function<TSource, TResult> projection;

    /***
     * Need this constructor for flatMap
     * @param input
     */
    protected MapIterator(Iterable input){
        super(input);

        // by default the projection is the id function
        this.projection = i -> (TResult)i;
    }

    public MapIterator(Iterable<TSource> source, Function<TSource, TResult> projection) {
        this(source);

        this.projection = projection;
    }

    @Override
    public boolean hasNext() {
        return source.hasNext();
    }

    @Override
    public TResult next() {
        return projection.apply((TSource)source.next());
    }
}
