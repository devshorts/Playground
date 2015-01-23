package com.devshorts.enumerable.iterators;

import java.util.function.Predicate;

/**
 * Created with IntelliJ IDEA.
 * User: anton.kropp
 * Date: 11/11/13
 * Time: 4:54 PM
 * To change this template use File | Settings | File Templates.
 */
public class TakeWhileIterator<TSource> extends EnumerableIterator<TSource> {
    private Predicate<TSource> predicate;
    private TSource nextItem;

    public TakeWhileIterator(Iterable<TSource> results, Predicate<TSource> predicate) {
        super(results);
        this.predicate = predicate;
    }

    @Override
    public boolean hasNext(){
        if(source.hasNext()){
            nextItem = (TSource)source.next();

            return predicate.test(nextItem);
        }

        return false;
    }

    @Override
    public TSource next(){
        return nextItem;
    }
}
