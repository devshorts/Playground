import java.util.Iterator;
import java.util.stream.Stream;

public class StreamUtil<T>{
    public static <T> Stream<T> ofIterable(Iterable<T> iter){
        Iterator<T> iterator = iter.iterator();

        Stream.Builder<T> builder = Stream.builder();

        while(iterator.hasNext()){
            builder.add(iterator.next());
        }

        return builder.build();
    }
}
