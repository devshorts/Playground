import com.devshorts.enumerable.Enumerable;
import com.devshorts.enumerable.data.Yieldable;

import java.util.List;
import java.util.concurrent.ExecutionException;

import static java.util.Arrays.asList;

class Box<T>{
    public T elem;

    public Box(T elem){
        this.elem = elem;
    }
}

public class Main{

    public static void main(String[] arsg) throws InterruptedException, ExecutionException {

        List<String> t = asList("oooo", "ba", "baz", "booo");

        Enumerable<String> strings = Enumerable.init(t);

        Box<Integer> b = new Box(0);

        Enumerable<Integer> sGen = Enumerable.generate(() -> {

            if(b.elem < 1000){
                b.elem++;
                System.out.println("yielding" + b.elem);
                return Yieldable.yield((b.elem % 10));
            }
            else{
                System.out.println("breaking");

                return Yieldable.yieldBreak();
            }
        }, () -> b.elem = 0);

        System.out.println(sGen.distinct().iter(System.out::println).toList());
    }
}