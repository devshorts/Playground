package com.devshorts.enumerable.iterators;

import java.util.Iterator;

public class EnumerableIterator<TSource> implements Iterator<TSource> {
    protected Iterator<TSource> source;
    private Iterable<TSource> input;

    public EnumerableIterator(Iterable<TSource> input){
        this.input = input;

        reset();
    }

    protected void reset(){
        source = input.iterator();
    }

    @Override
    public boolean hasNext() {
        return source.hasNext();
    }

    @Override
    public TSource next() {
        return (TSource)source.next();
    }
}
