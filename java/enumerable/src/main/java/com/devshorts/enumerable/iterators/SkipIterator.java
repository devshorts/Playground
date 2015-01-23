package com.devshorts.enumerable.iterators;

/**
 * Created with IntelliJ IDEA.
 * User: anton.kropp
 * Date: 11/11/13
 * Time: 4:59 PM
 * To change this template use File | Settings | File Templates.
 */
public class SkipIterator<TSource> extends EnumerableIterator<TSource> {
    private int skipNum;

    public SkipIterator(Iterable<TSource> source, int n){
        super(source);
        skipNum = n;
    }

    @Override
    public boolean hasNext(){
        while(skipNum > 0 && source.hasNext()){
            source.next();
            skipNum--;
        }

        return source.hasNext();
    }
}
