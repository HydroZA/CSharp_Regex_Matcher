

using System.Collections;
using System.Linq;

namespace cw1
{
    public class Matcher
    {
        public bool nullable(Rexp r) => r switch
        { 
            ZERO _ => false,
            ONE _ => true,
            CHAR _ => false,
            ALT alt => nullable(alt.r1) || nullable(alt.r2),
            SEQ seq => nullable(seq.r1) && nullable(seq.r2),
            STAR _ => true,
            RANGE _ => false,
            PLUS pl => nullable(pl.r),
            OPTIONAL _ => true,
            NTIMES nt => nt.n == 0 || nullable(nt.r),
            UPTO _ => true,
            FROM frm => frm.n == 0 || nullable(frm.r),
            BETWEEN btw => btw.n == 0 || nullable(btw.r),
            NOT not => !(nullable(not.r)),
            CFUN _ => false,
            ALL => false
        };

        public Rexp der(char c, Rexp r) => r switch
        {
            ZERO _ => r,
            ONE _ => new ZERO(),
            CHAR ch => der(c, new CFUN(ch.c.ToString().ToHashSet())),
            //CHAR ch => (c == ch.c) ? (Rexp)new ONE() : new ZERO(),
            ALT alt => new ALT(der(c, alt.r1), der(c, alt.r2)),
            SEQ seq => (nullable(seq.r1)) switch
            {
                true => new ALT(new SEQ(der(c, seq.r1), seq.r2), der(c, seq.r2)),
                false => new SEQ(der(c, seq.r1), seq.r2)
            },
            STAR star => new SEQ(der(c, star.r), new STAR(star.r)),
            RANGE rng => der(c, new CFUN(rng.s)),
            PLUS plus => new SEQ(der(c, plus.r), new STAR(plus.r)),
            OPTIONAL opt => der(c, opt.r),
            NTIMES nt => nt.n == 0 ? (Rexp) new ZERO() : new SEQ(der(c, nt.r), new NTIMES(nt.r, nt.n - 1)),
            UPTO upto => upto.m switch
            {
                0 => new ZERO(),
                _ => new SEQ(der(c, upto.r), new UPTO(upto.r, upto.m - 1))
            },
            FROM frm => frm.n == 0 ? new SEQ(der(c, frm.r), new STAR(r)) : new SEQ(der(c, frm.r), new FROM(frm.r, frm.n - 1)),
            BETWEEN btwn => (btwn.r, btwn.n, btwn.m) switch
            {
                (_, _, 0) => new ZERO(),
                (_, _, 1) => der(c, btwn.r),
                (_, 0, _) => new SEQ(der(c, btwn.r), new BETWEEN(btwn.r, 0, btwn.m - 1)),
                (_, _, _) => new SEQ(der(c, btwn.r), new BETWEEN(btwn.r, btwn.n - 1, btwn.m - 1))
            },
            NOT not => new NOT(der(c, not.r)),
            //CFUN cf => cf.f(c) ? new ONE() : new ZERO(),
            CFUN cf => cf.f(c) switch
            {
                true => new ONE(),
                false => new ZERO()
            },
            ALL => der(c, new CFUN()),
            _ => throw new System.Exception("GOT UNKNOWN REXP")
        };

        public Rexp ders(string s, Rexp r) 
        {
            switch (s.Length)
            {
                case 0:
                    return r;
                default:
                {
                    char c = s[0];
                    s = s.Substring(1);
                    return ders(s, simp(der(c, r)));
                }
            }
        }

        public Rexp simp(Rexp r)
        {
            switch (r)
            {
                case ALT alt:
                    {
                        Rexp r1s = simp(alt.r1);
                        Rexp r2s = simp(alt.r2);
                        switch(r1s, r2s)
                        {
                            case (ZERO, _):
                                return r2s;
                            case (_, ZERO):
                                return r1s;
                            case (_, _):
                                if (r1s == r2s)
                                    return r1s;
                                else
                                    return new ALT(r1s, r2s);
                        }
                    }
                case SEQ seq:
                    {
                        Rexp r1s = simp(seq.r1);
                        Rexp r2s = simp(seq.r2);
                        switch (r1s, r2s)
                        {
                            case (ZERO, _):
                                return new ZERO();
                            case (_, ZERO):
                                return new ZERO();
                            case (ONE, _):
                                return r2s;
                            case (_, ONE):
                                return r1s;
                            case (_, _):
                                return new SEQ(r1s, r2s);
                        }
                    }
                default:
                    return r;
            }
        }

        public bool Match(Rexp r, string s)
        {
            return nullable(ders(s, r));
        }

        public int Size(Rexp r) => r switch
        {
            ZERO _ => 1,
            ONE _ => 1,
            CHAR _ => 1,
            ALT alt => 1 + Size(alt.r1) + Size(alt.r2),
            SEQ seq => 1 + Size(seq.r1) + Size(seq.r2),
            STAR star => 1 + Size(star.r)
        };
    }
}
