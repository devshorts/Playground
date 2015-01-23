package com.devshorts.enumerable.iterators;

/**
 * Created with IntelliJ IDEA.
 * User: anton.kropp
 * Date: 11/11/13
 * Time: 4:46 PM
 * To change this template use File | Settings | File Templates.
 */
public class TakeIterator<TSource> extends EnumerableIterator<TSource> {
    private int takeNum;

    public TakeIterator(Iterable<TSource> results, int n) {
        super(results);
        takeNum = n;
    }

    @Override
    public boolean hasNext() {
        if(takeNum > 0){
            takeNum--;
            return source.hasNext();
        }

        return false;
    }
}
