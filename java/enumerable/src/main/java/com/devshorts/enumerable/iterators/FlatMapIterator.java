package com.devshorts.enumerable.iterators;

import java.util.List;
import java.util.function.Function;

/**
 * Created with IntelliJ IDEA.
 * User: anton.kropp
 * Date: 11/11/13
 * Time: 1:04 PM
 * To change this template use File | Settings | File Templates.
 */
public class FlatMapIterator<TSource, TResult> extends MapIterator<TSource, TResult> {
    private Function<TSource, List<TResult>> flatMapper;

    public FlatMapIterator(Iterable<TSource> source, Function<TSource, List<TResult>> flatMapper) {
        super(source);
        this.flatMapper = flatMapper;
    }

    private List<TResult> _bufferedResult;
    private Integer idx = 0;

    @Override
    public boolean hasNext() {
        if(_bufferedResult == null && source.hasNext()){
            _bufferedResult = flatMapper.apply((TSource)source.next());

            idx = 0;

            return true;
        }

        if(_bufferedResult != null){
            if(idx < _bufferedResult.size()){
                return true;
            }

            _bufferedResult = null;

            return hasNext();
        }

        return false;
    }


    public TResult next() {
        TResult item = _bufferedResult.get(idx);
        idx++;
        return item;
    }
}
